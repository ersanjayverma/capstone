package com.capstone.Arogya.dto;

import com.capstone.Arogya.model.QuestionType;
import lombok.*;

import java.util.Set;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class QuestionDto {

    private Long id;
    private String prompt;
    private QuestionType type;
    private boolean required;

    // Include MCQ options if type == MCQ
    private Set<QuestionOptionDto> options;

}
