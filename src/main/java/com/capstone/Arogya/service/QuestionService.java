package com.capstone.Arogya.service;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.dto.SubmitAnswerDto;
import com.capstone.Arogya.model.*;
import com.capstone.Arogya.repository.*;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Instant;
import java.util.List;
import java.util.stream.Collectors;

@RequiredArgsConstructor
@Service
public class QuestionService {

    private final QuestionRepository questionRepository;
    private final QuestionOptionRepository optionRepository;
    private final AnswerRepository answerRepository;

    @Transactional
    public AnswerDto submitAnswer(SubmitAnswerDto dto) {
        Question q = questionRepository.findById(dto.getQuestionId())
                .orElseThrow(() -> new IllegalArgumentException("Question not found: " + dto.getQuestionId()));

        // validate by type
        if (q.getType() == QuestionType.STRING && (dto.getAnswerText() == null || dto.getAnswerText().isBlank()) && q.isRequired()) {
            throw new IllegalArgumentException("Text answer required");
        }
        if (q.getType() == QuestionType.INT && dto.getAnswerInt() == null && q.isRequired()) {
            throw new IllegalArgumentException("Integer answer required");
        }
        if (q.getType() == QuestionType.MCQ && dto.getSelectedOptionId() == null && q.isRequired()) {
            throw new IllegalArgumentException("MCQ selection required");
        }

        // Optionally: update existing answer instead of always creating new.
        // For now: always insert new row. To update, findByUserIdAndQuestionId and update if present.

        Answer a = Answer.builder()
                .userId(dto.getUserId())
                .question(q)
                .answeredAt(Instant.now())
                .build();

        if (q.getType() == QuestionType.STRING) {
            a.setAnswerText(dto.getAnswerText());
        } else if (q.getType() == QuestionType.INT) {
            a.setAnswerInt(dto.getAnswerInt());
        } else if (q.getType() == QuestionType.MCQ) {
            QuestionOption opt = optionRepository.findById(dto.getSelectedOptionId())
                    .orElseThrow(() -> new IllegalArgumentException("Option not found: " + dto.getSelectedOptionId()));
            if (!opt.getQuestion().getId().equals(q.getId())) {
                throw new IllegalArgumentException("Option does not belong to the question");
            }
            a.setSelectedOption(opt);
        }

        Answer saved = answerRepository.save(a);
        return toDto(saved);
    }

    public List<AnswerDto> getAnswersForUser(Long userId) {
        return answerRepository.findByUserId(userId).stream().map(this::toDto).collect(Collectors.toList());
    }

    public List<AnswerDto> getAnswersForQuestion(Long questionId) {
        return answerRepository.findByQuestionId(questionId).stream().map(this::toDto).collect(Collectors.toList());
    }

    private AnswerDto toDto(Answer a) {
        return AnswerDto.builder()
                .id(a.getId())
                .userId(a.getUserId())
                .questionId(a.getQuestion().getId())
                .questionType(a.getQuestion().getType())
                .answerText(a.getAnswerText())
                .answerInt(a.getAnswerInt())
                .selectedOptionId(a.getSelectedOption() != null ? a.getSelectedOption().getId() : null)
                .selectedOptionText(a.getSelectedOption() != null ? a.getSelectedOption().getText() : null)
                .answeredAt(a.getAnsweredAt())
                .build();
    }
}
