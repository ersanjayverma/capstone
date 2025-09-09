package com.capstone.Arogya.repository;

import com.capstone.Arogya.model.Answer;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AnswerRepository extends JpaRepository<Answer, Long> {
    List<Answer> findByUserId(Long userId);
    List<Answer> findByQuestionId(Long questionId);
    List<Answer> findByUserIdAndQuestionId(Long userId, Long questionId);
}
